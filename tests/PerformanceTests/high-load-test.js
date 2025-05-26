import http from "k6/http";
import { check, sleep } from "k6";

// Define test options
export let options = {
  stages: [
    { duration: "1m", target: 100 }, // Ramp-up to 100 users over 1 minute
    { duration: "3m", target: 100 }, // Stay at 100 users for 3 minutes
    { duration: "1m", target: 0 }, // Ramp-down to 0 users over 1 minute
  ],
};

export default function () {
  const url = "http://localhost:5297/api/sms/simulate";

  // Generate unique NationalId and Name for each virtual user
  const nationalId = `ID${__VU}${__ITER}`; // Unique NationalId based on Virtual User ID and Iteration
  const name = `User${__VU}${__ITER}`; // Unique Name based on Virtual User ID and Iteration

  // Construct the registration message
  const messageContent = `REGISTER ${nationalId} ${name}`;

  // Payload for the API request
  const payload = JSON.stringify({
    From: `+23486${__VU}${__ITER}`, // Unique phone number for each VU and iteration
    Body: messageContent,
  });

  // HTTP headers
  const params = {
    headers: { "Content-Type": "application/json" },
  };

  // Send the POST request
  const res = http.post(url, payload, params);

  // Validate the response
  check(res, {
    "is status 200": (r) => r.status === 200,
    "response contains success message": (r) =>
      r.body.includes("Registration successful"),
  });

  sleep(1); // Simulate user think time
}
