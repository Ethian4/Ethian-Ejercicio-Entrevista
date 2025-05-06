# ✅ Validador de Paréntesis Balanceados con base de datos en Firebase

Este proyecto en C# permite validar si una cadena de texto tiene correctamente balanceados los símbolos de apertura y cierre

Además, guarda automáticamente el valor ingresado por el usuario junto con el resultado en una base de datos de Firebase. 

---
PUEDEN VER LOS RESULTADOS DE LA BASE DE DATOS CONECTADA A UNA PAGINA WEB HTML EN EL SIGUIENTE LINK: https://ethian4.github.io/Ethian-Ejercicio-Entrevista/
---

## 📌 Tecnologías utilizadas

- Lenguaje: C#
- Base de datos: Firebase

---

## 💻 Código fuente

```csharp
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

    // Clase para poder serializar los datos
    class ResultadoEntrada
    {
        public string Entrada { get; set; }
        public bool Resultado { get; set; }
        public string Fecha { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // Función para validar paréntesis
    static bool EstaBalanceado(string s)
    {
        // Pila para definir valores que abren los paréntesis.
        var stack = new Stack<char>();

        // Dictionary que une los valores de cierre con los que abren
        var pares = new Dictionary<char, char> { { ')', '(' }, { ']', '[' }, { '}', '{' } };

        /* Búsqueda de valores c. Si encuentra valor que abre, se agrega a la pila para poder verificar si se cierra después.
         * El else If se ejecuta cuando el valor ingresado por el usuario pertenece a un valor de cierre.
         * Si tiene valor de cierre, verifica la pila, si la pila es 0 significa por lógica que no se ha tenido un valor que abre el paréntesis, por lo que será False.
         * Stack.pop ayuda a verificar que correspondan los valores de cierre a los que se abrieron.
        */
        foreach (var c in s)
        {
            // Si el carácter no es uno de los válidos, muestra un mensaje de error y detiene el programa
            if (c != '(' && c != ')' && c != '[' && c != ']' && c != '{' && c != '}')
            {
                Console.WriteLine("Caracter no válido");
                return false; // Retorna false y termina el método inmediatamente
            }

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

    // Función Main del programa. Valores pedidos al usuario y resultados. 
    static async Task Main()
    {
        Console.WriteLine("Ingresar una cadena de paréntesis:");
        string entrada = Console.ReadLine();

        // Si los caracteres no son válidos, no continua con el siguiente código
        bool resultado = EstaBalanceado(entrada);

        // Si el resultado es false, no se guarda en Firebase
        if (resultado == false)
        {
            return; // Detiene la ejecución si no es válido
        }

        Console.WriteLine($"¿Balanceado? {resultado}");

        var datos = new ResultadoEntrada { Entrada = entrada, Resultado = resultado };
        await GuardarEnFirebase(datos);
    }
}
