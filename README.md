# Mag

## Overview
Just a docker snippet for minimal .NET image (`runtime-deps:8.0-jammy-chiseled`) with `IHost` abstraction

## Run
```bash
git clone --depth=1 https://github.com/dandriano/Mag.git && \
cd Mag && \
docker build . -t mag && \
docker run -it mag
```

## More
1. Problem [overview](https://habr.com/ru/companies/lamoda/articles/807179/)