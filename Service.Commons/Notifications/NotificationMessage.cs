namespace Service.Commons.Notifications;

public class NotificationMessage
{
    public static class Common
    {
        public static readonly string UnexpectedError = "Erro inesperado!";
        public static readonly string ValidationError = "Ocorreram um ou mais erros de validação!";
        public static readonly string RequestListRequired = "Lista da requisição não pode estar vazia!";
    }

    public static class Fatura
    {
        public static readonly string HasOneItem = "Fatura deve ter pelo menos um item!";
        public static readonly string InvalidDueDate = "Data de vencimento não pode ser anterior a hoje!";
        public static readonly string NotFound = "Fatura não encontrada!";
        public static readonly string IsOverDue = "Fatura vencida!";
        public static readonly string IsClosed = "Impossível adicionar ou remover itens de uma fatura fechada!";
        public static readonly string NotForClient = "Nenhuma fatura para esse cliente!";
        public static readonly string NotForDate = "Nenhuma fatura para esse mes do ano!";
        public static readonly string AlreadyIsClosed = "Fatura já foi fechada!";
    }

    public static class FaturaItem
    {
        public static readonly string NotChecked = "Valores acima de 1000 precisam de confiramção!";
        public static readonly string InvalidOrder = "Ordem deve ser um multiplo de 10!";
        public static readonly string InvalidValue = "Valor deve ser positivo!";
        public static readonly string NotFound = "Item da fatura não encontrado!";
    }
}
