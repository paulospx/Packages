# A method to load a CSV file into a SQL Server database using pandas and pyodbc
# pip install azure-storage-blob
# pip install pandas
# pip install pyodbc

import pandas as pd
from azure.storage.blob import BlobServiceClient
import pyodbc

def load_csv_to_mssql(csv_file, connection_string, table_name):
    """
    Load a CSV file into a SQL Server database table.

    Parameters:
    csv_file (str): Path to the CSV file.
    connection_string (str): Connection string for the SQL Server database.
    table_name (str): Name of the table to load data into.

    Returns:
    None
    """
    # Read the CSV file into a DataFrame
    df = pd.read_csv(csv_file)

    # Create a connection to the SQL Server database
    conn = pyodbc.connect(connection_string)


    # Load the DataFrame into the SQL Server table
    df.to_sql(table_name, conn, if_exists='replace', index=False)

    # Close the connection
    conn.close()

# A method to read the csv files an Azure Storage Location 
def read_csv_from_azure(azure_path):
    """
    Read a CSV file from an Azure Storage location.

    Parameters:
    azure_path (str): Path to the CSV file in Azure Storage.

    Returns:
    pd.DataFrame: DataFrame containing the CSV data.
    """
    # Assuming the Azure path is accessible via a URL
    df = pd.read_csv(azure_path)
    return df


# A method to list all CSV files on a Azure Storage Location using azure.storage.blob.BlobServiceClient

def list_csv_files_in_azure_storage(container_name, connection_string):
    """
    List all CSV files in an Azure Storage container.

    Parameters:
    container_name (str): Name of the Azure Storage container.
    connection_string (str): Connection string for the Azure Storage account.

    Returns:
    list: List of CSV file names in the specified container.
    """
    blob_service_client = BlobServiceClient.from_connection_string(connection_string)
    container_client = blob_service_client.get_container_client(container_name)

    csv_files = []
    for blob in container_client.list_blobs():
        if blob.name.endswith('.csv'):
            csv_files.append(blob.name)

    return csv_files
