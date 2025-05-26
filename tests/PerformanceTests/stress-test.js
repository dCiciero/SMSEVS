import http from "k6/http";
import { check, sleep } from "k6";

export let options = {
  stages: [
    { duration: "2m", target: 50 }, // Ramp-up to 50 users
    { duration: "2m", target: 100 }, // Ramp-up to 100 users
    { duration: "2m", target: 200 }, // Ramp-up to 200 users
    { duration: "2m", target: 0 }, // Ramp-down to 0 users
  ],
};

export default function () {
  const url = "http://localhost:5297/api/sms/simulate";

    // Generate unique NationalId and Name for each virtual user
    const nationalId = `ID${__VU}${__ITER}`; // Unique NationalId based on Virtual User ID and Iteration
    const name = `User${__VU}${__ITER}`;     // Unique Name based on Virtual User ID and Iteration

    // Construct the registration message
    const messageContent = `REGISTER ${nationalId} ${name}`;

      
    const payload = JSON.stringify({
      From: `+23486${__VU}${__ITER}`, // Unique phone number for each VU and iteration
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
