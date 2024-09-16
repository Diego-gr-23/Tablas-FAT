class FATFiles {
    public string Name { get; set;}
    public string Content { get; set; }
    public int Size => Content.Length;
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set;}
    public bool RecycleBinFlag { get; set; }
    public DateTime? DeletionDate { get; set; }


    public FATFiles(string name, string content){
        Name = name;
        Content = content;
        CreationDate = DateTime.Now;
        ModificationDate = DateTime.Now;
        RecycleBinFlag = false;

    }

}
