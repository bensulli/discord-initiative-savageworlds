FROM ubuntu:latest
RUN apt-get update; apt-get upgrade -y; apt-get install -y wget libicu-dev
RUN wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN DEBIAN_FRONTEND="noninteractive" apt-get -y install tzdata
RUN apt-get update; apt-get install -y apt-transport-https
RUN wget https://dot.net/v1/dotnet-install.sh; chmod +x dotnet-install.sh
RUN ./dotnet-install.sh -c 5.0
ENV PATH=$PATH:/root/.dotnet
ENV DOTNET_ROOT=/root/.dotnet
COPY . .
RUN dotnet publish
ARG DECK
ARG TOKEN
CMD cd DiscordInitiative/bin/Debug/net5.0/publish && ./DiscordInitiative --token="${TOKEN}" --deck="${DECK}"
