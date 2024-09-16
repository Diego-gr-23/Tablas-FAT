class FatFiles {
    private static List<FATFiles> files = new List<FATFiles>();
    static void Main(string[] args)
    {
        while(true){
            Console.WriteLine("1- Crear un archivo");
            Console.WriteLine("2- Listar un archivo");
            Console.WriteLine("3- Abrir un archivo");
            Console.WriteLine("4- Modificar un archivo");
            Console.WriteLine("5- Eliminar un archivo");
            Console.WriteLine("6- Recuperar un archivo");
            Console.WriteLine("7- Salir");

            var option = Console.ReadLine();
            Console.Clear();

            if (option == "1"){
                Console.WriteLine("Nombre del Archivo: ");
                string fileName = Console.ReadLine();

                Console.WriteLine("Ingrese el contenido del archivo (Cuando termine presiona ESC):");
                string content = ReadUntilEscape();

                FATFiles file = new FATFiles(fileName, content);
                file.Add(file)

            }

        }
    }
}