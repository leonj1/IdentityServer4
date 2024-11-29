// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Endpoints;
using IdentityServer4.Endpoints.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.Endpoints.EndSession
{
    public class EndSessionCallbackEndpointTests
    {
        private const string Category = "End Session Callback Endpoint";

        private StubEndSessionRequestValidator _stubEndSessionRequestValidator = new StubEndSessionRequestValidator();
        private EndSessionCallbackEndpoint _subject;
        private DefaultHttpContext _context;

        public EndSessionCallbackEndpointTests()
        {
            _context = new DefaultHttpContext();
            _subject = new EndSessionCallbackEndpoint(
                _stubEndSessionRequestValidator,
                new LoggerFactory().CreateLogger<EndSessionCallbackEndpoint>());
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_with_invalid_http_method_should_return_405()
        {
            _context.Request.Method = "POST";

            var result = await _subject.ProcessAsync(_context);

            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_with_valid_request_should_return_success()
        {
            _context.Request.Method = "GET";
            _stubEndSessionRequestValidator.Result.IsError = false;

            var result = await _subject.ProcessAsync(_context);

            var endSessionResult = result as EndSessionCallbackResult;
            endSessionResult.Should().NotBeNull();
            endSessionResult.Response.IsError.Should().BeFalse();
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_with_error_should_return_error_result()
        {
            _context.Request.Method = "GET";
            _stubEndSessionRequestValidator.Result.IsError = true;
            _stubEndSessionRequestValidator.Result.Error = "some_error";

            var result = await _subject.ProcessAsync(_context);

            var endSessionResult = result as EndSessionCallbackResult;
            endSessionResult.Should().NotBeNull();
            endSessionResult.Response.IsError.Should().BeTrue();
            endSessionResult.Response.Error.Should().Be("some_error");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_with_null_validator_should_throw()
        {
            var subject = new EndSessionCallbackEndpoint(
                null,
                new LoggerFactory().CreateLogger<EndSessionCallbackEndpoint>());

            await Assert.ThrowsAsync<ArgumentNullException>(() => 
                subject.ProcessAsync(_context));
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_with_null_logger_should_throw()
        {
            Assert.Throws<ArgumentNullException>(() => new EndSessionCallbackEndpoint(
                _stubEndSessionRequestValidator,
                null));
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_with_empty_query_should_return_error()
        {
            _context.Request.Method = "GET";
            _context.Request.QueryString = new QueryString("");
            _stubEndSessionRequestValidator.Result.IsError = true;
            _stubEndSessionRequestValidator.Result.Error = "invalid_request";

            var result = await _subject.ProcessAsync(_context);

            var endSessionResult = result as EndSessionCallbackResult;
            endSessionResult.Should().NotBeNull();
            endSessionResult.Response.IsError.Should().BeTrue();
            endSessionResult.Response.Error.Should().Be("invalid_request");
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task ProcessAsync_with_invalid_query_should_return_error()
        {
            _context.Request.Method = "GET";
            _context.Request.QueryString = new QueryString("?invalid=value");
            _stubEndSessionRequestValidator.Result.IsError = true;
            _stubEndSessionRequestValidator.Result.Error = "invalid_request";

            var result = await _subject.ProcessAsync(_context);

            var endSessionResult = result as EndSessionCallbackResult;
            endSessionResult.Should().NotBeNull();
            endSessionResult.Response.IsError.Should().BeTrue();
            endSessionResult.Response.Error.Should().Be("invalid_request");
        }
    }
}
