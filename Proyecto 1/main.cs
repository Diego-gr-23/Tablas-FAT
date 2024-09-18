class MainProgram{
    static void Main(string[] args)
    {
        FatFiles.LoadFATTable();

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
              FatFiles.Creatfile();
            }
            else if (option == "2"){
                FatFiles.ListFiles();
            }
            else if (option == "3"){
                FatFiles.OpenFile();
            }
            else if(option == "4"){
                FatFiles.ModifyFile();
            }
            else if(option == "5"){
                FatFiles.DeleteFile();
            }
            else if(option == "6"){
                FatFiles.RecoverFile();
            }
            else if(option == "7"){
                break;
            }
            else if(option == ""){
                Console.WriteLine("Opcion no valida.");
            }
        }
    }
}