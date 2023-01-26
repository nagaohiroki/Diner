using System.Collections.Generic;
public class EntryTicket
{
	public int entryNumber { get; set; }
	public string userId { get; set; }
	public ulong networkObjectId { get; set; }
	public Player player { get; set; }
}
public class EntryTickets
{
	Dictionary<int, EntryTicket> tickets = new Dictionary<int, EntryTicket>();
}
