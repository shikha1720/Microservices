namespace ProductAPI.RabbitmqService
{
    public interface IMessageProducer
    {
        public void SendingMessage<T>(T message);
    }
}
