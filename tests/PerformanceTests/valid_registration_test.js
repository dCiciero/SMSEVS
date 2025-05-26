import http from "k6/http";
import { check } from "k6";

export default function () {
  const url = "http://localhost:5297/api/sms/simulate";
  const payload = JSON.stringify({
    From: "+234521368773",
    Body: "REGISTER TN764296 John Doe",
  });

  const params = {
    headers: { "Content-Type": "application/json" },
  };

  const res = http.post(url, payload, params);
  console.log(`Response: ${res.status} - ${res.body}`);
  

  check(res, {
    "is status 200": (r) => r.status === 200,
    "response contains success message": (r) =>
      r.body.includes("Registration successful"),
  });
}
