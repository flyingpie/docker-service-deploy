# Setup Swarm

With at least a single node running and Docker installed, we need to setup a **Docker Swarm Cluster**.

This is quite easy, and can be done with a single command (replace the IP address with that of the node).

**First Node**
```bash
docker swarm init --advertise-addr 192.168.1.25
```

The IP is not strictly necessary, but especially when running on a host with multiple IP's, this prevents issues with Docker
choosing the wrong address and multiple nodes not being able to talk to one another.

Obtain a **join token** using the following command:
```bash
docker swarm join-token worker
```

For all the other nodes, run the following command to connect to the first node (replace the IP address with that of the node):

**All Other Nodes**
```bash
docker swarm join <token> --advertise-addr 192.168.1.26
```

Deployments can only be done from **manager** nodes. It's advisable to have either 3 or 5 manager nodes in a cluster.
