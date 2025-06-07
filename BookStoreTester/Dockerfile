# Stage 1: Build the Blazor application
# Uses the .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container to the application's root.
# IMPORTANT: This Dockerfile assumes Render's "Root Directory" for the service
# is correctly set to your Blazor project folder (e.g., 'BookStoreTester/').
# This means that when Docker builds, the 'BookStoreTester' folder is the build context root.
WORKDIR /app

# Copy ALL content from the build context (which is 'BookStoreTester/' on your host,
# assuming Render's Root Directory is correctly set) to the current WORKDIR /app in the container.
# This ensures that your 'BookStoreTester.csproj' and all other source files are present in /app.
# Ensure you have a '.dockerignore' file in 'BookStoreTester/' with 'Dockerfile' inside it.
COPY . .

# Restore project dependencies.
# 'dotnet restore' will now find 'BookStoreTester.csproj' in the current WORKDIR (/app),
# as it was copied by the 'COPY . .' command.
RUN dotnet restore

# Publish the application to an 'out' directory within the current WORKDIR (/app).
RUN dotnet publish -c Release -o out

# Stage 2: Serve the Blazor application
# Uses the ASP.NET runtime image for a smaller, production-ready image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime stage.
WORKDIR /app

# Copy the published output from the 'build' stage to the final runtime location.
# It was created at '/app/out' in the previous stage.
COPY --from=build /app/out ./

# Set the ASP.NET Core URL to listen on the port provided by Render.
# Render automatically sets the $PORT environment variable for web services.
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose the default port (8080) for ASP.NET Core apps within the container.
# Render will map its external port to this internal container port.
EXPOSE 8080

# Command to run the application when the container starts.
# 'BookStoreTester.dll' is the name of your compiled application DLL.
ENTRYPOINT ["dotnet", "BookStoreTester.dll"]
