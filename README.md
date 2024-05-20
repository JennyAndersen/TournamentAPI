# Tournament API

Tournament API is a web API for managing participant data and determining champions based on their total race time across different race types.

## Features

- Upload participant data file
- Parse and validate participant data from a file
- Calculate total race time for participants
- Determine champions based on the shortest total race time
- Returns the champions

## Technologies Used

- ASP.NET Core 8
- MediatR

## Getting Started

### Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or any other preferred IDE

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/JennyAndersen/TournamentAPI.git
2. Navigate to the cloned repository folder
3. Restore dependencies and build the project
4. Run the aplication

### Test the Endpoint
Using Swagger
Open Swagger UI:

In your local development environment, navigate to Swagger UI. The URL is typically https://localhost:port/swagger/index.html.

Locate the endpoint designed for uploading participant data files.

1. Click the "Try it out"
2. In the file input field, choose your local txtfile that you want to use by clicking on "Välj fil" and then "Open".
3. Click on the "Execute" button and observe the response body 

Please note that you might need to configure your Swagger UI to support file uploads depending on your specific setup.
