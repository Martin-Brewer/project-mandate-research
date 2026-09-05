#!/usr/bin/env python3
"""Independent black-box Project Mandate v0.1 containment verifier.

This implementation intentionally uses only the Python standard library and does not
import or execute any Project Mandate .NET verifier code. It exists to test whether
the published profile semantics are independently implementable in another language.
"""

from __future__ import annotations

import argparse
import json
from http.server import BaseHTTPRequestHandler, ThreadingHTTPServer
from typing import Any

PROFILE_TYPE = "project_mandate_authority"
ALLOWED_FIELDS = {
    "type",
    "actions",
    "resources",
    "purpose",
    "jurisdictions",
    "per_transaction_limit",
    "human_approval_above",
    "authority_state",
}
MONEY_FIELDS = {"currency", "amount"}


def _string_set(value: Any, field: str) -> set[str]:
    if not isinstance(value, list) or not value:
        raise ValueError(f"{field}_invalid")
    if any(not isinstance(item, str) or not item.strip() for item in value):
        raise ValueError(f"{field}_invalid")
    return set(value)


def _money(value: Any, field: str) -> tuple[str, float | int]:
    if not isinstance(value, dict) or set(value) != MONEY_FIELDS:
        raise ValueError(f"{field}_invalid")
    currency = value.get("currency")
    amount = value.get("amount")
    if not isinstance(currency, str) or not currency.strip():
        raise ValueError(f"{field}_currency_invalid")
    if isinstance(amount, bool) or not isinstance(amount, (int, float)) or amount < 0:
        raise ValueError(f"{field}_amount_invalid")
    return currency, amount


def _validate_authority(value: Any) -> dict[str, Any]:
    if not isinstance(value, dict):
        raise ValueError("authority_not_object")
    if set(value) != ALLOWED_FIELDS:
        raise ValueError("authority_fields_invalid")
    if value.get("type") != PROFILE_TYPE:
        raise ValueError("authority_type_invalid")
    if not isinstance(value.get("purpose"), str) or not value["purpose"].strip():
        raise ValueError("purpose_invalid")
    if not isinstance(value.get("authority_state"), str) or not value["authority_state"].strip():
        raise ValueError("authority_state_invalid")

    _string_set(value.get("actions"), "actions")
    _string_set(value.get("resources"), "resources")
    _string_set(value.get("jurisdictions"), "jurisdictions")
    _money(value.get("per_transaction_limit"), "per_transaction_limit")
    _money(value.get("human_approval_above"), "human_approval_above")
    return value


def validate_containment(parent_value: Any, child_value: Any) -> tuple[bool, list[str]]:
    """Apply Project Mandate authority profile v0.1 fail-closed containment rules."""
    try:
        parent = _validate_authority(parent_value)
    except ValueError as exc:
        return False, [f"parent_{exc}"]

    try:
        child = _validate_authority(child_value)
    except ValueError as exc:
        if str(exc) == "authority_type_invalid" and isinstance(child_value, dict):
            return False, ["type_changed"]
        return False, [f"child_{exc}"]

    reasons: list[str] = []

    if child["type"] != parent["type"]:
        reasons.append("type_changed")
    if not _string_set(child["actions"], "actions").issubset(_string_set(parent["actions"], "actions")):
        reasons.append("actions_broadened")
    if not _string_set(child["resources"], "resources").issubset(_string_set(parent["resources"], "resources")):
        reasons.append("resources_broadened")
    if child["purpose"] != parent["purpose"]:
        reasons.append("purpose_changed")
    if not _string_set(child["jurisdictions"], "jurisdictions").issubset(
        _string_set(parent["jurisdictions"], "jurisdictions")
    ):
        reasons.append("jurisdictions_broadened")

    parent_tx_currency, parent_tx_amount = _money(parent["per_transaction_limit"], "per_transaction_limit")
    child_tx_currency, child_tx_amount = _money(child["per_transaction_limit"], "per_transaction_limit")
    if child_tx_currency != parent_tx_currency:
        reasons.append("per_transaction_currency_changed")
    elif child_tx_amount > parent_tx_amount:
        reasons.append("per_transaction_limit_broadened")

    parent_approval_currency, parent_approval_amount = _money(parent["human_approval_above"], "human_approval_above")
    child_approval_currency, child_approval_amount = _money(child["human_approval_above"], "human_approval_above")
    if child_approval_currency != parent_approval_currency:
        reasons.append("approval_currency_changed")
    elif child_approval_amount > parent_approval_amount:
        reasons.append("approval_threshold_broadened")

    if child["authority_state"] != parent["authority_state"]:
        reasons.append("authority_state_changed")

    return not reasons, reasons


class Handler(BaseHTTPRequestHandler):
    server_version = "ProjectMandatePythonConformance/0.1"

    def log_message(self, fmt: str, *args: object) -> None:
        print(f"{self.address_string()} - {fmt % args}")

    def _write_json(self, status: int, body: dict[str, Any]) -> None:
        payload = json.dumps(body, separators=(",", ":")).encode("utf-8")
        self.send_response(status)
        self.send_header("Content-Type", "application/json")
        self.send_header("Content-Length", str(len(payload)))
        self.end_headers()
        self.wfile.write(payload)

    def _read_request_body(self) -> bytes:
        transfer_encoding = self.headers.get("Transfer-Encoding", "").lower()
        if "chunked" in transfer_encoding:
            chunks: list[bytes] = []
            while True:
                size_line = self.rfile.readline().strip()
                if not size_line:
                    raise ValueError("chunk_size_missing")
                chunk_size_text = size_line.split(b";", 1)[0]
                try:
                    chunk_size = int(chunk_size_text, 16)
                except ValueError as exc:
                    raise ValueError("chunk_size_invalid") from exc
                if chunk_size == 0:
                    while True:
                        trailer = self.rfile.readline()
                        if trailer in (b"\r\n", b"\n", b""):
                            break
                    break
                chunk = self.rfile.read(chunk_size)
                if len(chunk) != chunk_size:
                    raise ValueError("chunk_body_incomplete")
                chunks.append(chunk)
                line_end = self.rfile.read(2)
                if line_end != b"\r\n":
                    raise ValueError("chunk_terminator_invalid")
            return b"".join(chunks)

        content_length = self.headers.get("Content-Length")
        if content_length is None:
            raise ValueError("content_length_missing")
        length = int(content_length)
        if length <= 0:
            raise ValueError("request_body_empty")
        return self.rfile.read(length)

    def do_GET(self) -> None:  # noqa: N802
        if self.path == "/health":
            self._write_json(200, {"service": "python-verifier", "profile": PROFILE_TYPE, "version": "0.1", "status": "ok"})
            return
        self._write_json(404, {"error": "not_found"})

    def do_POST(self) -> None:  # noqa: N802
        if self.path != "/conformance/v0.1/containment":
            self._write_json(404, {"error": "not_found"})
            return

        try:
            body = json.loads(self._read_request_body())
            if not isinstance(body, dict) or set(body) != {"parent", "child"}:
                raise ValueError("request_shape_invalid")
            allowed, reasons = validate_containment(body["parent"], body["child"])
            self._write_json(200, {"allowed": allowed, "reasons": reasons})
        except (ValueError, json.JSONDecodeError) as exc:
            self._write_json(400, {"allowed": False, "reasons": [str(exc)]})


def main() -> None:
    parser = argparse.ArgumentParser(description="Independent Project Mandate v0.1 containment verifier")
    parser.add_argument("--host", default="127.0.0.1")
    parser.add_argument("--port", type=int, default=34004)
    args = parser.parse_args()

    server = ThreadingHTTPServer((args.host, args.port), Handler)
    print(f"Project Mandate Python verifier listening on http://{args.host}:{args.port}")
    server.serve_forever()


if __name__ == "__main__":
    main()
