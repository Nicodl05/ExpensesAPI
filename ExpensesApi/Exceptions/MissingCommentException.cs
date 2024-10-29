namespace ExpensesAPI.Exceptions
{
    public class MissingCommentException : Exception
    {
        public MissingCommentException()
            : base("Comment is mandatory.") { }
    }
}
