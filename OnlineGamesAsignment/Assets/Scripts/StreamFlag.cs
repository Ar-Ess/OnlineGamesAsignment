
public class StreamFlag
{
	uint flag = 0;

	public StreamFlag(uint flag)
	{
		this.flag = flag;
	}

	// Set an specific flag from slot 0 to 7
	void Set(ushort index, bool state)
	{
		// Flags only have 8 slots available
		if (!(index >= 0 && index < 8)) return;

		if (Get(index) == state) return;

		uint mod = 1;
		mod = mod << index;

		flag = flag ^ mod;
	}

	// Set the whole flag
	void Set(uint flag) { this.flag = flag; }

	// Get an specific flag
	bool Get(ushort index)
	{
		// Flags only have 8 slots available
		if (!(index >= 0 && index < 8)) return false;

		uint mod = 1;
		mod = mod << index;

		mod = flag & mod;

		return (mod != 0);
	}

	// Inverts the state of all the flags
	void Invert() 
	{ 
		flag = ~flag; 
	}

	bool IsAnyTrue() 
	{ 
		return flag != 0; 
	}

	void Clear() 
	{ 
		flag = 0; 
	}
}
