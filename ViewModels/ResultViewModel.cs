namespace Blog.ViewModels
{
	public class ResultViewModel<T>
	{
        //Retorna a saída
        public ResultViewModel(T data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }

        //Recebe acertos
        public ResultViewModel(T data)
        {
            Data = data;
        }

        //Recebe erros bad request (string list)
        public ResultViewModel(List<string> errors)
        {
            Errors = errors;
        }

        //Recebe erros not found (string)
        public ResultViewModel(string error)
        {
            Errors.Add(error);
        }

        public T Data { get; private set; }
        
        public List<string> Errors { get; private set; } = new();
    }
}
