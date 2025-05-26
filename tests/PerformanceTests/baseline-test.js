import http from "k6/http";
import { check, sleep } from "k6";

export let options = {
  stages: [
    { duration: "30s", target: 50 }, // Ramp-up to 50 users
    { duration: "1h", target: 50 }, // Sustain 50 users for 1 hour
    { duration: "20s", target: 0 }, // Ramp-down
  ],
};

const phoneNumbers = [
  "+447596058072",
  
];
export default function () {
  //   const url = "http://localhost:5297/api/sms/receive";
  const twilio_url = "https://59a7-37-60-75-172.ngrok-free.app/api/sms/receive";  // this is the webhook URL
  
  const url = "http://localhost:5297/api/sms/simulate";
  const phoneNumber = phoneNumbers[(__VU - 1) % phoneNumbers.length];

  const nationalId = `ID${__VU}${__ITER}`; // Unique NationalId based on Virtual User ID and Iteration
  const name = `User${__VU}${__ITER}`; // Unique Name based on Virtual User ID and Iteration


  const messageContent = `REGISTER ${nationalId} ${name}`;
  const payload = JSON.stringify({
    From: `+12345678${__VU}`,
    Body: messageContent,
  });


  const params = {
    headers: { "Content-Type": "application/json" },
  };

  const res = http.post(url, payload, params);

  check(res, {
    "is status 200": (r) => r.status === 200,
  });

  sleep(1); // Simulate 1-second delay between requests
}
