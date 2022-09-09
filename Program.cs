using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Grammar;
using Interpreter.Lang;

internal class Program
{
    private static void Main(string[] args)
    {
        //LEXER
        //### input        
        //var inputStream = new AntlrFileStream("input.lang");
        var inputStream = new AntlrFileStream(args[0]);
        //### lexer
        var lexer = new LangLexer(inputStream); // implementa os lexers

        //PARSER
        //### tokens
        var tokenStream = new BufferedTokenStream(lexer); // carrega e guarda e buffers
        //### parser
        var parser = new LangParser(tokenStream); // criando o parser

        //### error listener
        var errorListener = new LangErrorListener(); // remove anteriores
        parser.RemoveErrorListeners();
        parser.AddErrorListener(errorListener);
        //### error handling
        //parser.ErrorHandler = new BailErrorStrategy(); // para quando identificar o primeiro erro
        parser.ErrorHandler = new DefaultErrorStrategy(); //acha varios erros 


        //### semantic listener
        var semanticListener = new SemanticLangListener();
        parser.RemoveParseListeners();
        parser.AddParseListener(semanticListener);

        

        //### parse
        IParseTree? tree = null;
        try
        {
            tree = parser.prog();
            if (errorListener.HasErrors){ // verificando se tem erros caso tenha imprima os erros 
                Console.WriteLine("Errors!");
                errorListener.ErrorMessages.ForEach(e => Console.WriteLine(e));
                tree = null;
                
                
            }
            if (semanticListener.HasErrors){
                Console.WriteLine("Semantic Errors!");
                semanticListener.ErrorMessages.ForEach(e => Console.WriteLine(e));
                tree = null;
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

       // Console.WriteLine("##### FUNCTIONS");
       // semanticListener.Functions.Keys.ToList().ForEach(f => Console.WriteLine(f));

        //### execute
        if (tree != null)
        {
            var interpreter = new LangInterpreter(semanticListener.Functions);
            interpreter.Visit(tree);
        }

    }
}