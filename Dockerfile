#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["AutoReservation/AutoReservation.csproj", "AutoReservation/"]
COPY ["Service/Service.csproj", "Service/"]
COPY ["Model/Model.csproj", "Model/"]
COPY ["Repository/Repository.csproj", "Repository/"]
RUN dotnet restore "AutoReservation/AutoReservation.csproj"
COPY . .
WORKDIR "/src/AutoReservation"
RUN dotnet build "AutoReservation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoReservation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoReservation.dll"]

docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password=123456 -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/test.pfx -v D:\temp:/https/ autoresevation