using System;
using System.Text.RegularExpressions;

namespace Parametros2 {
    class Program {
        static String reserv = @"^(byte|short|int|long|float|double|bool|char|String|Array|Byte|Short|Integer|Long|Float|Double|Boolean|Character|[a-zA-Z_$][a-zA-Z_$0-9]*)";

        static void Main(string[] args) {
            int sw = 1;
            while (sw == 1) {
                Console.WriteLine("Escriba los parametros que desea validar separados por comas");
                Console.Write("Parametros >> ");
                String ParamValid = Console.ReadLine().Trim();

                String pattern = @"\s*,\s*";
                String[] elems = System.Text.RegularExpressions.Regex.Split(ParamValid, pattern);

                bool noHayError = true, hayIdsRepetidos = false, idUsaPalabraReserv = false;
                int elemsAVerificar = 0;
                String[] id = new String[elems.Length]; // Vector de identificadores

                for (int i = 0; i < elems.Length; i++) {
                    elems[i] = elems[i].Trim();
                    if (elems[i].Length > 4) {
                        if (elems[i].Substring(0, 5) == "final") { elems[i] = elems[i].Substring(5).Trim(); }
                    }

                    if (elems[i].IndexOf("[") == -1 && elems[i].IndexOf("]") == -1) {
                        if (elems[i].IndexOf(" ") > -1) { // Ej: "Hola a" => ["Hola", "a"]
                            String[] valor = System.Text.RegularExpressions.Regex.Split(elems[i].Trim(), @"\s+");
                            if (valor.Length == 2) { id[elemsAVerificar++] = valor[1]; }
                        }
                    }
                    else { // Entra aqui si el parametro es un vector o una matriz
                        int corchUltimoIdx = elems[i].LastIndexOf("]");
                        if (corchUltimoIdx > -1) {
                            if (corchUltimoIdx == elems[i].Length - 1) { // Ej: Clase a[][], int b [] []
                                int primerCorch = elems[i].IndexOf("[");
                                if (primerCorch > -1) {
                                    String[] valor = System.Text.RegularExpressions.Regex.Split(elems[i].Substring(0, primerCorch).Trim(),  @"\s+");
                                    if (valor.Length == 2) { id[elemsAVerificar++] = valor[1]; }
                                }
                            } else if(corchUltimoIdx < elems[i].Length - 1) {
                                id[elemsAVerificar++] = elems[i].Substring(corchUltimoIdx + 1);
                            }
                        }
                    }
                }

                Regex v = new Regex(@"^(byte|short|int|long|float|double|bool|char|String|Array|Byte|Short|Integer|Long|Float|Double|Boolean|Character)");
                for (int y = 0; y < elemsAVerificar; y++) {
                    if (v.IsMatch(id[y].Trim())) { idUsaPalabraReserv = true; } // Esta usando una palabra reservada?
                }

                // Verificar que identificadores no sean iguales
                for (int y = 0; y < elemsAVerificar - 1; y++) {
                    for (int j = y + 1; j < elems.Length; j++) {
                        if (id[y] == id[j]) { hayIdsRepetidos = true; }
                    }
                }

                for (int i = 0; i < elems.Length; i++) { // Ej.: final int a
                    if (!verificarParam(elems[i])) { Console.WriteLine("Error en el parametro " + (i + 1)); noHayError = false; }
                }

                if (hayIdsRepetidos) { Console.WriteLine("Error, hay identificadores repetidos"); }

                if (idUsaPalabraReserv) { Console.WriteLine("Error, hay identificadores usando palabras reservadas"); }

                if (noHayError && !hayIdsRepetidos && !idUsaPalabraReserv) { Console.WriteLine("Los parametros estan bien escritos"); }

                Console.Write("¿Desea continuar? (1: si/ 0: no): ");
                if (Console.ReadLine() != "1") { sw = 0; }
            }

        }

        static bool verificarParam(String elem) {
            Regex r2 = new Regex(reserv + @"\s*(\[\s*\]\s*)+\s*[a-zA-Z_$][a-zA-Z_$0-9]*$"); // int[] a, int[]a, int[   ]a, int [] [] [] a
            if (r2.IsMatch(elem)) { return true; }

            Regex r3 = new Regex(reserv + @"\s+[a-zA-Z_$][a-zA-Z_$0-9]*\s*(\[\s*\]\s*)*$");// int a[] [] [], int b[][][]
            if (r3.IsMatch(elem)) { return true; }

            return false;
        }
    }
}
