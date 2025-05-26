import json
import matplotlib.pyplot as plt
import pandas as pd
import os

# Function to load k6 JSON results
def load_k6_results(file_path):
    with open(file_path, 'r') as f:
        data = [json.loads(line) for line in f if line.strip()] #json.load(f)
    return data

# Function to extract metrics from k6 results
# def extract_metrics(data, metric_name):
#     if metric_name not in data['metrics']:
#         raise ValueError(f"Metric '{metric_name}' not found in the results.")
    
#     metric_data = data['metrics'][metric_name]['values']
#     timestamps = [entry['time'] for entry in metric_data]
#     values = [entry['value'] for entry in metric_data]
#     return timestamps, values


def extract_metrics(data, metric_name):
    timestamps = []
    values = []

    for item in data:
        if item.get("metric") == metric_name and item.get("type") == "Point":
            time = item["data"]["time"]
            value = item["data"]["value"]
            timestamps.append(time)
            values.append(value)

    return timestamps, values


# Function to plot a graph
def plot_graph(timestamps, values, title, xlabel, ylabel, output_file=None):
    plt.figure(figsize=(10, 6))
    plt.plot(timestamps, values, label=title, color='blue', marker='o')
    plt.xlabel(xlabel)
    plt.ylabel(ylabel)
    plt.title(title)
    plt.grid(True)
    plt.legend()
    if output_file:
        plt.savefig(output_file)
    plt.show()

# Main function to process and plot graphs for different tests
def main():
    # Specify the test files and their descriptions
    test_files = {
        "../results/burst-test-with-otp.json": "Burst Test",
        "../results/high-load-test-with-otp.json": "High Load Test",
        "../results/stress-test-with-otp.json": "Stress Test",
        "../results/step-load-test-with-otp.json": "Step Load Test",
        # "baseline-test-no-otp.json": "Baseline Test"
    }

    # base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "results"))
    # test_files = {
    #     os.path.join(base_dir, "results", "burst-test-no-otp.json"): "Burst Test",
    #     os.path.join(base_dir, "results", "high-load-test-no-otp.json"): "High Load Test",
    #     os.path.join(base_dir, "results", "stress-test-no-otp.json"): "Stress Test",
    #     os.path.join(base_dir, "results", "step-load-test-no-otp.json"): "Step Load Test"
    # }

    # Metric to extract (e.g., http_req_duration for response time)
    metric_name = "http_req_duration"

    for file_name, test_name in test_files.items():
        try:
            # Load the k6 results
            data = load_k6_results(file_name)

            # Extract timestamps and values for the specified metric
            timestamps, values = extract_metrics(data, metric_name)

            # Convert timestamps to a readable format (optional)
            timestamps = pd.to_datetime(timestamps)
            # timestamps = pd.to_datetime(timestamps, unit='ms')

            # Plot the graph
            plot_graph(
                timestamps,
                values,
                title=f"{test_name} - {metric_name.replace('_', ' ').title()}",
                xlabel="Time",
                ylabel="Duration (ms)",
                output_file=f"../results/graphs/{test_name.lower().replace(' ', '_')}_graph.png"
            )
        except Exception as e:
            print(f"Error processing {file_name}: {e}")

# Run the script
if __name__ == "__main__":
    main()