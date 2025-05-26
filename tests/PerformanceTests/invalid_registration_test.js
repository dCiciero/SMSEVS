import http from "k6/http";
import { check } from "k6";

export default function () {
  const url = "http://localhost:5297/api/sms/simulate";
  const payload = JSON.stringify({
    From: "+1234567890",
    Body: "INVALIDCOMMAND",
  });

  const params = {
    headers: { "Content-Type": "application/json" },
  };

  const res = http.post(url, payload, params);

  // console.log(`Response status: ${res.status}`);
  // console.log(`Response body: ${res.body}`);
  

  check(res, {
    "is status 400": (r) => r.status === 400,
    "response contains error message": (r) =>
      r.body.includes("Invalid format"),
  });
}
