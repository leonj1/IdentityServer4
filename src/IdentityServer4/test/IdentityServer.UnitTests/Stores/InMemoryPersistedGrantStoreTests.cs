using IdentityServer4.Models;
using IdentityServer4.Stores;
using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System.Linq;

namespace IdentityServer.UnitTests.Stores
{
    public class InMemoryPersistedGrantStoreTests
    {
        InMemoryPersistedGrantStore _subject;

        public InMemoryPersistedGrantStoreTests()
        {
            _subject = new InMemoryPersistedGrantStore();
        }

        [Fact]
        public async Task Store_should_persist_value()
        {
            {
                var item = await _subject.GetAsync("key1");
                item.Should().BeNull();
            }

            await _subject.StoreAsync(new PersistedGrant() { Key = "key1" });

            {
                var item = await _subject.GetAsync("key1");
                item.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task GetAll_should_filter()
        {
            await _subject.StoreAsync(new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1", SessionId = "session1" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key2", SubjectId = "sub1", ClientId = "client2", SessionId = "session1" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key3", SubjectId = "sub1", ClientId = "client1", SessionId = "session2" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key4", SubjectId = "sub1", ClientId = "client3", SessionId = "session2" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key5", SubjectId = "sub1", ClientId = "client4", SessionId = "session3" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key6", SubjectId = "sub1", ClientId = "client4", SessionId = "session4" });

            await _subject.StoreAsync(new PersistedGrant() { Key = "key7", SubjectId = "sub2", ClientId = "client4", SessionId = "session4" });



            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key1", "key2", "key3", "key4", "key5", "key6" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key7" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub3"
            }))
            .Select(x => x.Key).Should().BeEmpty();

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client1"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key1", "key3" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client2"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key2" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client3"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key4" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client4"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key5", "key6" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client5"
            }))
            .Select(x => x.Key).Should().BeEmpty();

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2",
                ClientId = "client1"
            }))
            .Select(x => x.Key).Should().BeEmpty();

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2",
                ClientId = "client4"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key7" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub3",
                ClientId = "client1"
            }))
            .Select(x => x.Key).Should().BeEmpty();

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client1",
                SessionId = "session1"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key1" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client1",
                SessionId = "session2"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key3" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client1",
                SessionId = "session3"
            }))
            .Select(x => x.Key).Should().BeEmpty();

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client2",
                SessionId = "session1"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key2" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client2",
                SessionId = "session2"
            }))
            .Select(x => x.Key).Should().BeEmpty();

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client4",
                SessionId = "session4"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key6" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2",
                ClientId = "client4",
                SessionId = "session4"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key7" });

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2",
                ClientId = "client4",
                SessionId = "session1"
            }))
            .Select(x => x.Key).Should().BeEmpty();

            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub2",
                ClientId = "client4",
                SessionId = "session5"
            }))
            .Select(x => x.Key).Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveAll_should_filter()
        {
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1"
                });
                (await _subject.GetAsync("key1")).Should().BeNull();
                (await _subject.GetAsync("key2")).Should().BeNull();
                (await _subject.GetAsync("key3")).Should().BeNull();
                (await _subject.GetAsync("key4")).Should().BeNull();
                (await _subject.GetAsync("key5")).Should().BeNull();
                (await _subject.GetAsync("key6")).Should().BeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub2"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().BeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub3"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client1"
                });
                (await _subject.GetAsync("key1")).Should().BeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().BeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client2"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().BeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client3"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().BeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client4"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().BeNull();
                (await _subject.GetAsync("key6")).Should().BeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client5"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub2",
                    ClientId = "client1"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client4"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().BeNull();
                (await _subject.GetAsync("key6")).Should().BeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub3",
                    ClientId = "client1"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client1",
                    SessionId = "session1"
                });
                (await _subject.GetAsync("key1")).Should().BeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client1",
                    SessionId = "session2"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().BeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client1",
                    SessionId = "session3"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client2",
                    SessionId = "session1"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().BeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client2",
                    SessionId = "session2"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub1",
                    ClientId = "client4",
                    SessionId = "session4"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().BeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub2",
                    ClientId = "client4",
                    SessionId = "session4"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().BeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub2",
                    ClientId = "client4",
                    SessionId = "session1"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub2",
                    ClientId = "client4",
                    SessionId = "session5"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
            {
                await Populate();
                await _subject.RemoveAllAsync(new PersistedGrantFilter
                {
                    SubjectId = "sub3",
                    ClientId = "client1",
                    SessionId = "session1"
                });
                (await _subject.GetAsync("key1")).Should().NotBeNull();
                (await _subject.GetAsync("key2")).Should().NotBeNull();
                (await _subject.GetAsync("key3")).Should().NotBeNull();
                (await _subject.GetAsync("key4")).Should().NotBeNull();
                (await _subject.GetAsync("key5")).Should().NotBeNull();
                (await _subject.GetAsync("key6")).Should().NotBeNull();
                (await _subject.GetAsync("key7")).Should().NotBeNull();
            }
        }

        [Fact]
        public async Task Remove_should_remove_grant()
        {
            await _subject.StoreAsync(new PersistedGrant() { Key = "key1" });
            (await _subject.GetAsync("key1")).Should().NotBeNull();

            await _subject.RemoveAsync("key1");
            
            (await _subject.GetAsync("key1")).Should().BeNull();
        }

        [Fact]
        public async Task GetAll_should_filter_by_type()
        {
            await _subject.StoreAsync(new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1", Type = "type1" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key2", SubjectId = "sub1", ClientId = "client1", Type = "type2" });
            
            (await _subject.GetAllAsync(new PersistedGrantFilter
            {
                SubjectId = "sub1",
                ClientId = "client1",
                Type = "type1"
            }))
            .Select(x => x.Key).Should().BeEquivalentTo(new[] { "key1" });
        }

        [Fact]
        public async Task GetAll_should_handle_empty_filter()
        {
            await _subject.StoreAsync(new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key2", SubjectId = "sub1", ClientId = "client2" });
            
            var grants = await _subject.GetAllAsync(new PersistedGrantFilter());
            grants.Select(x => x.Key).Should().BeEquivalentTo(new[] { "key1", "key2" });
        }

        [Fact]
        public async Task Store_should_update_existing_grant()
        {
            var grant = new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1", Data = "data1" };
            await _subject.StoreAsync(grant);
            
            var updatedGrant = new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1", Data = "data2" };
            await _subject.StoreAsync(updatedGrant);

            var result = await _subject.GetAsync("key1");
            result.Data.Should().Be("data2");
        }

        [Fact]
        public async Task Remove_should_handle_non_existing_grant()
        {
            // Should not throw
            await _subject.RemoveAsync("nonexistent_key");
        }

        [Fact]
        public async Task GetAll_should_handle_null_filter()
        {
            await _subject.StoreAsync(new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1" });
            
            var grants = await _subject.GetAllAsync(null);
            grants.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveAll_should_handle_null_filter()
        {
            await _subject.StoreAsync(new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1" });
            
            // Should not throw
            await _subject.RemoveAllAsync(null);
            
            var result = await _subject.GetAsync("key1");
            result.Should().NotBeNull();
        }

        private async Task Populate()
        {
            await _subject.StoreAsync(new PersistedGrant() { Key = "key1", SubjectId = "sub1", ClientId = "client1", SessionId = "session1" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key2", SubjectId = "sub1", ClientId = "client2", SessionId = "session1" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key3", SubjectId = "sub1", ClientId = "client1", SessionId = "session2" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key4", SubjectId = "sub1", ClientId = "client3", SessionId = "session2" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key5", SubjectId = "sub1", ClientId = "client4", SessionId = "session3" });
            await _subject.StoreAsync(new PersistedGrant() { Key = "key6", SubjectId = "sub1", ClientId = "client4", SessionId = "session4" });

            await _subject.StoreAsync(new PersistedGrant() { Key = "key7", SubjectId = "sub2", ClientId = "client4", SessionId = "session4" });
        }
    }
}
