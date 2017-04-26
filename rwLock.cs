using System;
namespace HelloWorldApplication
{
	class rwClass
	{
		private int rcount=0;
		private int wcount=0;
		private int wwcount=0;
		static void read()
		{
			//1.no writer and no wait writer
			if(wcount==0&&wwcount==0)  
			{
				lock(this){
					rcount++;	
				}
				thread.sleep();
			}
			//2.has writer
			else if(wcount!=0)
			{
				//wait for message
			}
			else
			{
				//still wait	
			}
			
		}
		
		static void write()
		{
			//1.	no reader
			if(rcount==0)
			{
				lock(this){
					wcount=1;
				}
				thread.sleep();
				lock(this){
					wcount=0;
				}
				//give out message(write finished)
				
			}
			
			//2. has readers. and no writer
				//wait for read finished.
			
			//3. has no reader but a writer
				//wait for write finished
			
			//4.
		}
		
	}
   class HelloWorld
   {
      static void Main(string[] args)
      {
         /* 我的第一个 C# 程序*/
         Console.WriteLine("Hello World!");
         Console.ReadKey();
      }
   }
}
