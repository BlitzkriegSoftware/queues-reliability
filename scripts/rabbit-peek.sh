#!/bin/bash
# Dequeues item
# notice port points to the redirected admin port (the /api)
python.exe rabbitmqadmin.py get queue=myQueue -u guest -p guest  -P 8080 