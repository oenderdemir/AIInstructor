#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update \
&& apt-get install -y fontconfig --no-install-recommends \
libgdiplus libc6-dev tzdata \
&& ln -fs /usr/share/zoneinfo/Europe/Istanbul /etc/localtime \
&& dpkg-reconfigure -f noninteractive tzdata \
&& rm -rf /var/lib/apt/lists/*



RUN ln -s /mnt /app/mnt


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AIInstructor.csproj", "."]
RUN dotnet restore "AIInstructor.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "AIInstructor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AIInstructor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AIInstructor.dll"]