# messaging-frontend
A sample project using RabbitMQ to power an ASP.NET Twitter clone, running on SignalR.

Demo code for the webinar 'Powering Front-End Apps with Messaging'

The webinar was hosted by [@ParticularSW](http://particular.net/), the makers of NServiceBus, ServiceMatrix, ServicePulse etc. You can [check out the slide deck](http://www.slideshare.net/sixeyed/messaging-powered-front-ends), and the webinar recording will be coming soon.

It was presented on 25th September 2015 by me, [@EltonStoneman](http://blog.sixeyed.com), with help from @farmar and @MauroServienti.

# The Code

The demo solution is a simple Noticeboard Web App, where users can post mail. 

When you send a mail it gets broadcast to all connected users, and saved to a database. You get toast notifications confirming when those steps have been done.

The app uses ASP.NET SignalR as the channel between web server and clients, RabbitMQ as the messaging layer to power the workflow, and HBase to store data.

It demonstrates using pub-sub and request-response messaging patterns at both the RabbitMQ and SignalR layers.

## Usage

1. Start the dependencies - the easiest way is to install Docker (or Kitematic) and run a couple of containers, one for RabbitMQ and the other for HBase:

+ docker run -d -p 8080:8080 sixeyed/hbase-stargate
+ docker run -d -p 15672:15672 -p 5672:5672 rabbitmq:3-management

When you first run them, the images get pulled from the Docker Hub and that can take a while, but once you have the images locally, each container will start in a couple of seconds.

You don't need to configure anything in RabbitMQ or HBase, the solution does that when it runs.

2. Update config - replace the {x-IP-ADDRESS} values with the IP address of your Docker host in:

+ Sixeyed.MessagingPoweredFrontEnd.Handlers.Persistence\app.config 
+ Sixeyed.MessagingPoweredFrontEnd.Web\Web.config

3. Build the solution and run the console app to start the handler. Then open a couple of browser windows to simulate different users being logged in, in the Webinar I used Edge, Firefox and Chrome pointing to:

+ http://localhost/noticeboard?user=elton
+ http://localhost/noticeboard?user=mauro
+ http://localhost/noticeboard?user=sean

Send a mail from one window, and you'll see it appear in the others - and you get two toasts popping up, one to tell you the mail was broadcast, and the other to tell you the mail was saved.

## Verifying

The RabbitMQ container exposes the management interface, so you can browse to:

+ http://{YOUR-DOCKER-HOST}:15672*

with credentials *guest/guest* and check out the queues, exhchanges etc.

If you want to query the data in HBase, connect to the container and query the data in the *mail* table:

+ docker ps -> to get the {instance ID} of your HBase container
+ docker exec -it {instance ID} /opt/hbase/bin/hbase shell
+ scan 'mail'


## Next Steps

This Webinar focused on using messaging to power a front-end; the other webinars in the series covered [Handling Failures with Messaging](https://blog.sixeyed.com/handling-failures-with-messaging/), and [Scaling with Asynchronous Messaging](https://github.com/sixeyed/going-async).

Check out my @Pluralsight course, [Message Queue Fundamentals in .NET](http://www.pluralsight.com/courses/message-queue-fundamentals-dotnet) for more patterns, practices, and lots more queueing technologies.

I use HBase because it's super easy to run in a Docker container, but it's almost as easy to run in an Azure cluster which can handles thousands of requests per second. I cover that in another @Pluralsight course, [Real World Big Data in Azure](http://www.pluralsight.com/courses/real-world-big-data-microsoft-azure).

