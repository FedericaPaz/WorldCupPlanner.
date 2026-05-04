namespace Domain;

public class Group
{
    private char _groupLetter;
    
    public char GroupLetter
    {
        get => _groupLetter;
        set => _groupLetter = value;
    }
    
    public Group(char groupLetter)
    {
        if (groupLetter < 'A' || groupLetter > 'L') 
            throw new ArgumentException("El grupo debe ser una letra de la A a la L.");
        GroupLetter = groupLetter;
    }
}