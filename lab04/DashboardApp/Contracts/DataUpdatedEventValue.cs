using Prism.Events;

namespace Contracts;

public record DataUpdatedEventValue(string Data);
public class DataUpdatedEvent : PubSubEvent<DataUpdatedEventValue>;
