using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bio.Variant
{
    class BaseAndQualityAndPosition
    {
        public int Position;
        public byte Base 
        { 
            get{return BaseWithQuality.Base;}
        }
        public ushort InsertPositionPassedRefPosition;
        public bool PositionIsInsertion
        {
            get 
            {
                return InsertPositionPassedRefPosition > 0;            
            }
        }         
        public BaseAndQuality BaseWithQuality;
        public BaseAndQualityAndPosition(int position, int insertSizeRelativeToPosition, BaseAndQuality BandQ)
        {
            this.BaseWithQuality = BandQ;
            this.Position = position;
            this.InsertPositionPassedRefPosition = (ushort) insertSizeRelativeToPosition;
        }
    }
}
