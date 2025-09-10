# Generate a script that combines 2 csv files with the same row structure the filenames are passed as arguments
import sys
import pandas as pd

def combine_csv_files(file1, file2, output_file):
    # Read both CSV files
    df1 = pd.read_csv(file1)
    df2 = pd.read_csv(file2)
    
    # Concatenate the DataFrames
    combined_df = pd.concat([df1, df2], ignore_index=True)
    
    # Save the combined DataFrame to a new CSV file
    combined_df.to_csv(output_file, index=False)
    print(f"Combined CSV saved to {output_file}")

if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Usage: python captain_csv.py <file1.csv> <file2.csv> <output.csv>")
        sys.exit(1)
    
    file1 = sys.argv[1]
    file2 = sys.argv[2]
    output_file = sys.argv[3]
    
    combine_csv_files(file1, file2, output_file)




