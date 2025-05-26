import json

def summarize_k6_result(file_path):
    with open(file_path) as f:
        data = json.load(f)
    metrics = data['metrics']
    total_reqs = metrics['http_reqs']['count']
    success = metrics['checks']['passes']
    failures = metrics['checks']['fails']
    avg_rt = metrics['http_req_duration']['avg']
    p95_rt = metrics['http_req_duration']['p(95)']
    print(f"Total Requests: {total_reqs}")
    print(f"Successful Requests: {success} ({success/total_reqs*100:.1f}%)")
    print(f"Failed Requests: {failures} ({failures/total_reqs*100:.1f}%)")
    print(f"Average Response Time: {avg_rt}ms")
    print(f"95th Percentile Response Time: {p95_rt}ms")

# Example usage:
summarize_k6_result('results/burst-test-no-otp.json')