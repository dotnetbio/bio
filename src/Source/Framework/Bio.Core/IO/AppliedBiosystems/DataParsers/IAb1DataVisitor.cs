using Bio.IO.AppliedBiosystems.DataTypes;

namespace Bio.IO.AppliedBiosystems.DataParsers
{
   /// <summary>
   /// Parsing visitor for constructing the <see cref="Context"/> during a abi parse operation.  Each visit method handles a different known
   /// item within the abi file format.
   /// </summary>
   public interface IAb1DataVisitor
   {
       /// <summary>
       /// Parser context.
       /// </summary>
      IParserContext Context { get; }

       /// <summary>
       /// Visit byte item.
       /// </summary>
       /// <param name="item"></param>
      void Visit(ByteDataItem item);

      /// <summary>
      /// Visit char item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(CharDataItem item);

      /// <summary>
      /// Visit word item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(WordDataItem item);

      /// <summary>
      /// Visit short item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(ShortDataItem item);

      /// <summary>
      /// Visit long item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(LongDataItem item);

      /// <summary>
      /// Visit float item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(FloatDataItem item);

      /// <summary>
      /// Visit double item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(DoubleDataItem item);

      /// <summary>
      /// Visit date item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(DateDataItem item);

      /// <summary>
      /// Visit time item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(TimeDataItem item);

      /// <summary>
      /// Visit pstring item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(PStringDataItem item);

      /// <summary>
      /// Visit thumb item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(ThumbDataItem item);

      /// <summary>
      /// Visit bool item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(BoolDataItem item);

      /// <summary>
      /// Visit user item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(UserDataItem item);

      /// <summary>
      /// Visit cstring item.
      /// </summary>
      /// <param name="item"></param>
      void Visit(CStringDataItem item);
   }
}
