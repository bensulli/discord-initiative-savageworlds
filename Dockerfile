FROM ubuntu:latest
COPY . .
ARG token
ARG deck
RUN apt-get update; apt-get upgrade -y; apt-get install -y wget
RUN wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN DEBIAN_FRONTEND="noninteractive" apt-get -y install tzdata
RUN apt-get update; apt-get install -y apt-transport-https && apt-get update && apt-get install -y dotnet-sdk-5.0 dotnet-sdk-5.0 aspnetcore-runtime-5.0
RUN dotnet publish
CMD cd DiscordInitiative/bin/Debug/netcoreapp5.0/publish/; ./DiscordInitiative --token="$token" --deck="$deck"