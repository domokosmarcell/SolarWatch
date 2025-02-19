# SolarWatch

## About The Project

SolarWatch is a webpage designed for showing you the sunset/sunrise time of the provided city at the provided date using the given timezone information. 

## Built With

- **Backend**: [ASP.NET Core](https://dotnet.microsoft.com/en-us/apps/aspnet) (with Identity Framework and Entity Framework)
- **Frontend**: [React.js](https://reactjs.org/)
- **Containerization**: [Docker](https://www.docker.com/)

---


## Prerequisites

Make sure you have the following installed:

[![Docker][Docker]](https://www.docker.com/)

[![Node.js][Node.js]](https://nodejs.org/)

[![.NET 8 SDK][.NET]](https://dotnet.microsoft.com/)

---

## Start the App

0. Before you start the application go to the SolarWatch folder and search for a file called `appsettings.Development.json`. Create a `.env` file for yourself and outside of the `Logging` property copy everything from the appsettings to this file and replace the example data with your own. For the API key related to Geocoding API visit this page: https://openweathermap.org/. Create an account and you can ask for an API key (every instruction is on the site).


### With Terminal

1. Start the database using Docker:

  ```sh
    docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server
  ```

  - After the `-p` you have to specify port numbers in this order: exposed:internal
  - If you already have the official MSSql server image in your Docker than you can use this comand instead: `docker start <MSSql container id>`
  
2. Navigate to the backend directory and start the application backend:

  ```sh
    cd SolarWatch
    dotnet run --project SolarWatch.csproj --launch-profile https
  ```


<!--Links for logos! -->
[Docker]: https://img.shields.io/badge/Docker-blue?style=plastic&logo=docker&logoColor=darkblue
[Node.js]: https://img.shields.io/badge/Node.js-black?style=plastic&logo=nodedotjs&logoColor=green
[.NET]: https://img.shields.io/badge/.NET_8_SDK-darkblue?style=plastic&logo=dotnet&logoColor=white&labelColor=purple
