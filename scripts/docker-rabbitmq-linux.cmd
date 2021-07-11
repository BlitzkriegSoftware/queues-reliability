@REM See: https://hub.docker.com/_/rabbitmq/
docker stop some-rabbit
docker rm some-rabbit
docker pull rabbitmq:3-alpine
docker run -d --hostname my-rabbit --name some-rabbit -p 8080:15672 -p 5672:5672 rabbitmq:3-management-alpine