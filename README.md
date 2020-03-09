# Breaker.Net
## Build status
[![Build Status](https://travis-ci.org/lrAlval/Breaker.svg?branch=master)](https://travis-ci.org/lrAlval/Breaker)
## Overview

`Breaker.Net` is an implementation of the Circuit Breaker pattern for .NET with zero dependencies.

This pattern can help you monitor the performance of a remote resource and improve the stability and resilience of your application by isolating the communication with third-party services, so that your application won't be affected by their fails.
 
You can read more about [Martin Fowler](https://martinfowler.com/bliki/CircuitBreaker.html) and [on MSDN](https://docs.microsoft.com/en-us/azure/architecture/patterns/circuit-breaker)

## Example Usage

```csharp
// Initialize the circuit breaker
var settings = new CircuitBreakerConfig
{
    ResetTimeOut = TimeSpan.FromMilliseconds(40),
    InvocationTimeOut = TimeSpan.FromMilliseconds(250),
    FailuresThreshold = 2,
    SuccessThreshold = 1,
    TaskScheduler = TaskScheduler.Default
};
var circuitBreaker = new CircuitBreaker(settings);

try
{
    // perform a potentially fragile call through the circuit breaker
    circuitBreaker.ExecuteAsync(() => _fragileService.CallAsync);
}
catch (CircuitBreakerOpenException)
{
    // the service is unavailable, failover here
}
catch (Exception)
{
    // handle other unexpected exceptions
}

```

## Why?

There are [not so many of them][nuget-curcuitbreaker]. I didn't find any that would suit me, since I need to specify a separated TaskScheduler for the third-party service calls. [Polly][polly] seems the most mature from all of them but it has a locking nature and it doesn't provide a way to specify a separate `TaskScheduler` to execute actions.  The code provided on MSDN isn't production ready and just a piece of code. And so on so forth. So that the yet another library was born :construction: .

## Roadmap

 - Publish to Nuget
 - Adding more events to easily integrate with any analytics platform
 - Added some container support 

  [nuget-curcuitbreaker]: https://www.nuget.org/packages?q=circuit+breaker
  [polly]: https://github.com/michael-wolfenden/Polly
  [fowler]: http://martinfowler.com/bliki/CircuitBreaker.html
  [MSDN]: https://msdn.microsoft.com/en-us/library/dn589784.aspx
