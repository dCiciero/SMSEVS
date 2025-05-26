import http from "k6/http";
import { check, sleep } from "k6";

export let options = {
  stages: [
    { duration: "1m", target: 50 }, // Start with 50 users
    { duration: "1m", target: 100 }, // Increase to 100 users
    { duration: "1m", target: 200 }, // Increase to 200 users
    { duration: "1m", target: 300 }, // Increase to 300 users
    { duration: "1m", target: 400 }, // Increase to 400 users
    { duration: "1m", target: 500 }, // Peak at 500 users
    { duration: "1m", target: 0 }, // Ramp-down
  ],
};

export default function () {
  // const url = 'http://localhost:5000/api/sms/receive';
  const url = "http://localhost:5297/api/sms/simulate";

  const nationalId = `ID${__VU}${__ITER}`; // Unique NationalId based on Virtual User ID and Iteration
  const name = `User${__VU}${__ITER}`; // Unique Name based on Virtual User ID and Iteration

  // Construct the registration message
  const messageContent = `REGISTER ${nationalId} ${name}`;

  // Payload for the API request
  const payload = JSON.stringify({
    From: `+23480641${__VU}${__ITER}`, // Unique phone number for each VU and iteration
    Body: messageContent,
  });

  const params = {
    headers: { "Content-Type": "application/json" },
  };

  const res = http.post(url, payload, params);

  check(res, {
    "is status 200": (r) => r.status === 200,
  });

  sleep(1);
}
