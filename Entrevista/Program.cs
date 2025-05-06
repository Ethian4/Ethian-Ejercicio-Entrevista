// Programa Ejercicio Entrevista Ethian Emiliano Martinez Zafiro

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    // Direccion de mi base de datos de Firebase
    static readonly string firebaseUrl = "https://ejercicioethian-default-rtdb.firebaseio.com/validaciones.json";

    // Clase para poder serealizar los datos
    class ResultadoEntrada
    {
        public string Entrada { get; set; }
        public bool Resultado { get; set; }
        public string Fecha { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // Función para validar paréntesis
    static bool EstaBalanceado(string s)

    {   //Pila para definir valores que abren los parentesis.
        var stack = new Stack<char>();

        //Dictionary que unes los valores de cierre con los que abren
        var pares = new Dictionary<char, char> { { ')', '(' }, { ']', '[' }, { '}', '{' } };

        /*Busqueda de valores c. Si encuentra valor que abre, se agrega a la pila para poder verificar si se cierra despues.
         * El else If se ejecuta cuando el valor ingresado por el usuario pertenece a un valor de cierre.
         * Si tiene valor de cierre, verifica la pila, si la pila es 0 significa por logica que no se ha tenido un valor que abre el parantesis, por lo que sera False.
         * Stack.pop ayuda a verificar que correspondan los valores de cierre a los que se abrieron.
        */
        foreach (var c in s)
        {
            if (pares.ContainsValue(c))
                stack.Push(c);
            else if (pares.ContainsKey(c))
            {
                if (stack.Count == 0 || stack.Pop() != pares[c])
                    return false;
            }
        }

        return stack.Count == 0;
    }

    // Función para guardar en Firebase
    static async Task GuardarEnFirebase(ResultadoEntrada data)
    {
        using var client = new HttpClient();
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await client.PostAsync(firebaseUrl, content);
    }

    //Funcion Main del programa. Valores pedidos al usuario y resultados. 
    static async Task Main()
    {
        Console.WriteLine("Ingresar una cadena de parentesis:");
        string entrada = Console.ReadLine();

        bool resultado = EstaBalanceado(entrada);
        Console.WriteLine($"¿Balanceado? {resultado}");

        var datos = new ResultadoEntrada { Entrada = entrada, Resultado = resultado };
        await GuardarEnFirebase(datos);
    }
}
