using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Septerra.Core.AM;

namespace Septerra.Core
{
    public sealed class ImageMeta
    {
        private readonly XmlDescriptor _descriptor = new XmlDescriptor();

        public void SetAnimationType(Int32 animationType)
        {
            _descriptor.AnimationType = animationType;
        }

        public void SetAnimation(AMAnimation[] animations)
        {
            _descriptor.Animations = animations.SelectArray(a => new XmlAnimation(in a));
        }

        public void SetFrames(AMFrame[] frames)
        {
            _descriptor.Frames = frames.SelectArray(f => new XmlFrame(in f));
        }

        public Byte[] ToXml()
        {
            using (MemoryStream ms = new MemoryStream(8192))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlDescriptor));
                serializer.Serialize(ms, _descriptor);

                return ms.ToArray();
            }
        }

        public static (Int32 animationType, AMAnimation[] animations, AMFrame[] frames) FromXml(Byte[] array)
        {
            using (MemoryStream ms = new MemoryStream(array))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlDescriptor));
                XmlDescriptor descriptor = (XmlDescriptor)serializer.Deserialize(ms);

                AMAnimation[] animations = descriptor.Animations.SelectArray(x => x.ToAMAnimation());
                AMFrame[] frames = descriptor.Frames.SelectArray(x => x.ToAMFrame());

                return (descriptor.AnimationType, animations, frames);
            }
        }

        [XmlType]
        public sealed class XmlDescriptor
        {
            [XmlAttribute] public Int32 AnimationType;
            [XmlElement] public XmlAnimation[] Animations;
            [XmlElement] public XmlFrame[] Frames;

            public XmlDescriptor()
            {
            }
        }

        [XmlType]
        public sealed class XmlAnimation
        {
            [XmlElement] public List<XmlFrameReference> Frames;
            [XmlElement] public XmlAnimationShift[] Shifts;

            public XmlAnimation()
            {
            }

            public XmlAnimation(in AMAnimation a)
            {
                Frames = new List<XmlFrameReference>(AMAnimation.MaxFrames);
                for (Int32 i = 0; i < AMAnimation.MaxFrames; i++)
                {
                    ref AMFrameReference fr = ref a.GetFrames(i);
                    Frames.Add(new XmlFrameReference(in fr));
                }

                for (Int32 i = Frames.Count-1; i>= 1; i--)
                {
                    XmlFrameReference last = Frames[i];
                    XmlFrameReference previous = Frames[i - 1];

                    if (last.Count == 0 && last.Index == previous.Index + previous.Count)
                        Frames.RemoveAt(i);
                    else
                        break;
                }

                if (a.ShiftNumber > 0)
                {
                    Shifts = new XmlAnimationShift[a.ShiftNumber];
                    for (Int32 i = 0; i < a.ShiftNumber; i++)
                    {
                        ref AMAnimationShift shift = ref a.GetShift(i);
                        Shifts[i] = new XmlAnimationShift(in shift);
                    }
                }
            }

            public AMAnimation ToAMAnimation()
            {
                AMAnimation result = new AMAnimation();
                ToAMAnimation(ref result, Frames);
                ToAMAnimation(ref result, Shifts);
                return result;
            }

            private void ToAMAnimation(ref AMAnimation result, List<XmlFrameReference> frames)
            {
                Int32 index = 0;
                UInt16 last = UInt16.MaxValue;

                for (; index < frames.Count; index++)
                {
                    XmlFrameReference source = frames[index];
                    ref AMFrameReference target = ref result.GetFrames(index);

                    target.FrameIndex = source.Index;
                    target.FrameCount = source.Count;

                    last = (UInt16)(source.Index + source.Count);
                }

                for (; index < AMAnimation.MaxFrames; index++)
                {
                    ref AMFrameReference target = ref result.GetFrames(index);
                    target.FrameIndex = last;
                }
            }

            private void ToAMAnimation(ref AMAnimation result, XmlAnimationShift[] shifts)
            {
                if (shifts == null)
                    return;

                result.ShiftNumber = shifts.Length;

                for (Int32 i = 0; i < shifts.Length; i++)
                {
                    XmlAnimationShift source = shifts[i];
                    ref AMAnimationShift target = ref result.GetShift(i);

                    target.OX = source.OX;
                    target.OY = source.OY;
                }
            }
        }

        [XmlType]
        public sealed class XmlFrameReference
        {
            [XmlAttribute] public UInt16 Index;
            [XmlAttribute] public UInt16 Count;

            public XmlFrameReference()
            {
            }

            public XmlFrameReference(in AMFrameReference fr)
            {
                Index = fr.FrameIndex;
                Count = fr.FrameCount;
            }
        }

        [XmlType]
        public sealed class XmlAnimationShift
        {
            [XmlAttribute] public Int32 OX;
            [XmlAttribute] public Int32 OY;

            public XmlAnimationShift()
            {
            }

            public XmlAnimationShift(in AMAnimationShift shift)
            {
                OX = shift.OX;
                OY = shift.OY;
            }
        }

        [XmlType]
        public sealed class XmlFrame
        {
            [XmlAttribute] public UInt16 ImageIndex;
            [XmlAttribute] public UInt16 MaskImageIndex;
            [XmlAttribute] public UInt16 PalleteIndex;
            [XmlAttribute] public UInt16 FrameTime;
            [XmlAttribute] public Int16 MinusOX;
            [XmlAttribute] public Int16 MinusOY;
            [XmlAttribute] public Int16 OffsetOX;
            [XmlAttribute] public Int16 OffsetOY;
            [XmlAttribute] public Int16 PlusOX;
            [XmlAttribute] public Int16 PlusOY;
            [XmlAttribute] public Boolean FlipHorizontal;
            [XmlAttribute] public Boolean FlipVertical;
            [XmlAttribute] public UInt16 Flags;
            [XmlAttribute] public UInt16 AnimationMirrorIndexFrom0To4; // Araym's flying hands

            public XmlFrame()
            {
            }

            public XmlFrame(in AMFrame frame)
            {
                ImageIndex = frame.ImageIndex;
                MaskImageIndex = frame.MaskImageIndex;
                PalleteIndex = frame.PalleteIndex;
                FrameTime = frame.FrameTime;
                MinusOX = frame.MinusOX;
                MinusOY = frame.MinusOY;
                OffsetOX = frame.OffsetOX;
                OffsetOY = frame.OffsetOY;
                PlusOX = frame.PlusOX;
                PlusOY = frame.PlusOY;
                FlipHorizontal = frame.FlipHorizontal;
                FlipVertical = frame.FlipVertical;
                Flags = frame.Flags;
                AnimationMirrorIndexFrom0To4 = frame.AnimationMirrorIndexFrom0To4;
            }

            public AMFrame ToAMFrame()
            {
                var frame = this;

                return new AMFrame
                {
                    ImageIndex = frame.ImageIndex,
                    MaskImageIndex = frame.MaskImageIndex,
                    PalleteIndex = frame.PalleteIndex,
                    FrameTime = frame.FrameTime,
                    MinusOX = frame.MinusOX,
                    MinusOY = frame.MinusOY,
                    OffsetOX = frame.OffsetOX,
                    OffsetOY = frame.OffsetOY,
                    PlusOX = frame.PlusOX,
                    PlusOY = frame.PlusOY,
                    FlipHorizontal = frame.FlipHorizontal,
                    FlipVertical = frame.FlipVertical,
                    Flags = frame.Flags,
                    AnimationMirrorIndexFrom0To4 = frame.AnimationMirrorIndexFrom0To4,
                };
            }
        }
    }
}