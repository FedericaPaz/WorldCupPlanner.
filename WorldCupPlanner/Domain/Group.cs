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
        GroupLetter = groupLetter;
    }
}