namespace BlazorEnterpriseStarter.E2ETests.Infrastructure;

[CollectionDefinition(nameof(E2ECollection))]
public sealed class E2ECollection : ICollectionFixture<E2ETestHostFixture>, ICollectionFixture<PlaywrightFixture>;
