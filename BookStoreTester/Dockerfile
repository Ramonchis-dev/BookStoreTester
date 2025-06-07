# Stage 1: Build the Blazor application
# Uses the .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
# This is a temporary staging area for the source code.
WORKDIR /src

# Copy all content from the build context (where Dockerfile is located)
# to the working directory '/src' in the container.
# Since your Dockerfile is in 'BookStoreTester/', this will copy
# the contents of 'BookStoreTester/' (including .csproj) to '/src'.
COPY . .

# Set the working directory to the project folder inside the container
# This is crucial so 'dotnet restore' and 'dotnet publish' can find the .csproj
# Replace 'BookStoreTester' with the actual name of your project folder if different
WORKDIR /src/BookStoreTester

# Restore project dependencies
# 'dotnet restore' will now find 'BookStoreTester.csproj' in the current WORKDIR
RUN dotnet restore

# Publish the application to an 'out' directory
# -c Release: Build in Release configuration
# -o out: Output to a directory named 'out' within the current WORKDIR (/src/BookStoreTester)
RUN dotnet publish -c Release -o out

# Stage 2: Serve the Blazor application
# Uses the ASP.NET runtime image for a smaller, production-ready image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime stage
WORKDIR /app

# Copy the published output from the 'build' stage
# The 'out' directory from the previous stage is copied to '/app' in this stage.
COPY --from=build /src/BookStoreTester/out ./

# Set the ASP.NET Core URL to listen on the port provided by Render
# Render automatically sets the $PORT environment variable.
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose the default port (8080) for ASP.NET Core apps within the container.
# Render will map its external port to this internal container port.
EXPOSE 8080

# Command to run the application when the container starts
# 'BookStoreTester.dll' is the name of your compiled application DLL.
ENTRYPOINT ["dotnet", "BookStoreTester.dll"]
