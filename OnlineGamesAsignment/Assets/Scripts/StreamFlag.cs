
public class StreamFlag
{
	public uint flag = 0;

	public StreamFlag(uint flag)
	{
		this.flag = flag;
	}

	// Set an specific flag from slot 0 to 7
	public void Set(ushort index, bool state)
	{
		// Flags only have 8 slots available
		if (!(index >= 0 && index < 8)) return;

		if (Get(index) == state) return;

		uint mod = 1;
		mod = mod << index;

		flag = flag ^ mod;
	}

	// Set the whole flag
	public void Set(uint flag) { this.flag = flag; }

	// Get an specific flag
	public bool Get(ushort index)
	{
		// Flags only have 8 slots available
		if (!(index >= 0 && index < 8)) return false;

		uint mod = 1;
		mod = mod << index;

		mod = flag & mod;


		return (mod != 0);
	}

	// Inverts the state of all the flags
	public void Invert() 
	{ 
		flag = ~flag; 
	}

	public bool IsAnyTrue() 
	{ 
		return flag != 0; 
	}

	public void Clear() 
	{ 
		flag = 0; 
	}
}
