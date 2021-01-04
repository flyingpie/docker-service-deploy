# Overview

Welcome!

## What It Does

...
(image: "docker service create" + "docker service update" + JSON = docker service deploy)
...

Docker Service Deploy is a simple abstraction layer over the **docker service create** and **docker service update** commands.
Furthermore, it adds a declarative JSON format to describe the service to be deployed.

The whole thing comes as a command line tool and as a .Net library.

## An Alternative To Docker Stack

It's all quite similar (and heavily inspired by) [Docker Stacks](https://docs.docker.com/engine/swarm/stack-deploy/), and indeed, the tooling is set up to
be relatively easy to migrate from- and to.

Reasons for creating Docker Service Deploy (CAUTION: Opinions incoming):

**Conceptual differences, prone to personal preferences:**

- Only a **single service** per deployment item
- No new conceptual layer ("stacks") on top of services
- Environments (development, production, etc.) are built-in and (can be) part of the service spec
- Declarative service format as JSON instead of YAML

**Features that Docker Stack is missing (at the time of writing):**

- Doesn't support binding to the local "nat" network on Windows hosts ("host" ports won't work)
- When deploying "ingress" ports, containers cannot be accessed from the localhost
- No "attached" way of deploying, which means that during deployment, there is no way to wait for the deployment to complete (either successfully or otherwise)
- Stacks seem to only exist as a concept in the docker-cli, which means they are kinda hard to manage using the Engine API
- As a result of the previous points, there is no way to use the declarative Docker Compose format to deploy a Stack through the Engine API
