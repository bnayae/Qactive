﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Xml;
using Qactive;
using SharedLibrary;

namespace QbservableServer
{
  class AdvancedService
  {
    private static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port: 38246);

    public IDisposable Start(TraceSource trace)
    {
      var service = QbservableTcpServer.CreateService<IList<FeedServiceArgument>, FeedItem>(
        endPoint,
        new QbservableServiceOptions() { SendServerErrorsToClients = true, AllowExpressionsUnrestricted = true },
        request =>
          (from arguments in request.Do(args => ConsoleTrace.WriteLine(ConsoleColor.DarkCyan, "Advanced service received {0} arguments.", args.Count))
           from feed in arguments
           from _ in Observable.Timer(TimeSpan.Zero, TimeSpan.FromMinutes(1))
           from item in Observable.Using(() => new HttpClient(), client => client.GetStreamAsync(feed.Url).ToObservable().Select(feed => SyndicationFeed.Load(XmlReader.Create(feed))))
           select new FeedItem()
           {
             FeedUrl = feed.Url,
             Title = item.Title.Text,
             PublishDate = item.LastUpdatedTime
           })
          .Do(item => ConsoleTrace.WriteLine(ConsoleColor.Green, "Advanced service generated item: {0}", item.Title)));

      return service.Subscribe(
        terminatedClient =>
        {
          foreach (var ex in terminatedClient.Exceptions)
          {
            ConsoleTrace.WriteLine(ConsoleColor.Magenta, "Advanced service error: {0}", ex.SourceException.Message);
          }

          ConsoleTrace.WriteLine(ConsoleColor.Yellow, "Advanced client shutdown: " + terminatedClient.Reason);
        },
        ex => ConsoleTrace.WriteLine(ConsoleColor.Red, "Advanced fatal service error: {0}", ex.Message),
        () => Console.WriteLine("This will never be printed because a service host never completes."));
    }
  }
}