using Shaml.Assigners;
using Shaml.Tokens;

namespace Shaml
{
	public static class ShamlConverter
	{
		public static Node Parse(ReadOnlyMemory<char> buffer)
		{
			// Node googleApiNode = new(buffer)
			// {
			// 	Children =
			// 	{
			// 		{
			// 			/// Request
			// 			new StaticReference(new Mark(0, 6), buffer),
			// 			new Node(buffer)
			// 			{
			// 				Children =
			// 				{
			// 					/// Url: google.com
			// 					{
			// 						new StaticReference(new Mark(10, 12), buffer),
			// 						new Scalar(buffer) { Value = new Mark(15, 24) }
			// 					},
			// 					{
			// 						/// Query
			// 						new StaticReference(new Mark(27, 31), buffer),
			// 						new Node(buffer)
			// 						{
			// 							Children =
			// 							{
			// 								/// name: John
			// 								{
			// 									new StaticReference(new Mark(36, 39), buffer),
			// 									new Scalar(buffer) { Value = new Mark(42, 45) }
			// 								},
			// 								/// age: 18
			// 								{
			// 									
			// 									new StaticReference(new Mark(49, 51), buffer),
			// 									new Scalar(buffer) { Value = new Mark(54, 55) }
			// 								},
			// 							}
			// 						}
			// 					},
			// 					{
			// 						/// Headers
			// 						new StaticReference(new Mark(58, 64), buffer),
			// 						new Node(buffer)
			// 						{
			// 							Children =
			// 							{
			// 								/// SecFetchSite: site
			// 								{
			// 									new StaticReference(new Mark(69, 80), buffer),
			// 									new Scalar(buffer) { Value = new Mark() }
			// 								},
			// 								/// SecFetchDesc: user
			// 								{
			// 									new StaticReference(new Mark(90, 101), buffer),
			// 									new Scalar(buffer) { Value = new Mark() }
			// 								},
			// 								/// SecFetchMode: one
			// 								{
			// 									new StaticReference(new Mark(111, 122), buffer),
			// 									new Scalar(buffer) { Value = new Mark() }
			// 								},
			// 							}
			// 						}
			// 					},
			// 					// Body
			// 					{
			// 						new StaticReference(new Mark(130, 133), buffer),
			// 						new Scalar(buffer) { Value = new Mark(144, 164) }
			// 					},
			// 				}
			// 			}
			// 		},
			// 		{
			// 			/// User
			// 			new StaticReference(new Mark(204, 207), buffer),
			// 			new Node(buffer)
			// 			{
			// 				Children =
			// 				{
			// 					/// Name: John
			// 					{
			// 						new StaticReference(new Mark(211, 214), buffer),
			// 						new Scalar(buffer) { Value = new Mark(217, 220) }
			// 					},
			// 					/// Age: 18
			// 					{
			// 						new StaticReference(new Mark(223, 225), buffer),
			// 						new Scalar(buffer) { Value = new Mark(228, 229) }
			// 					},
			// 				}
			// 			}
			// 		},
			// 		{
			// 			/// List
			// 			new StaticReference(new Mark(232, 235), buffer),
			// 			new Node(buffer)
			// 			{
			// 				Children =
			// 				{
			// 					/// item1
			// 					{ new IndexReference(0, buffer), new Scalar(buffer) { Value = new Mark(241, 245) } },
			// 					/// item2
			// 					{ new IndexReference(1, buffer), new Scalar(buffer) { Value = new Mark(250, 254) } },
			// 					/// item3
			// 					{ new IndexReference(2, buffer), new Scalar(buffer) { Value = new Mark(259, 263) } },
			// 				}
			// 			}
			// 		}
			//
			// 	}
			// };

			
			Node primitiveTypeNode = new(buffer)
			{
				Children =
				{
					{ new StaticReference(new Mark(0, 11), buffer), new Scalar(buffer) { Value = new Mark(14, 16) } },
					{ new StaticReference(new Mark(19, 27), buffer), new Scalar(buffer) { Value = new Mark(30, 32) } },
					{ new StaticReference(new Mark(35, 48), buffer), new Scalar(buffer) { Value = new Mark(51, 54) } },
					{ new StaticReference(new Mark(57, 67), buffer), new Scalar(buffer) { Value = new Mark(70, 73) } },
					{ new StaticReference(new Mark(76, 88), buffer), new Scalar(buffer) { Value = new Mark(91, 94) } },
					{ new StaticReference(new Mark(97, 106), buffer), new Scalar(buffer) { Value = new Mark(109, 112) } },
					{ new StaticReference(new Mark(115, 126), buffer), new Scalar(buffer) { Value = new Mark(129, 138) } },
					{ new StaticReference(new Mark(141, 149), buffer), new Scalar(buffer) { Value = new Mark(152, 185) } },
					{ new StaticReference(new Mark(188, 198), buffer), new Scalar(buffer) { Value = new Mark(201, 210) } },
					{ new StaticReference(new Mark(213, 220), buffer), new Scalar(buffer) { Value = new Mark(223, 232) } },
					{ new StaticReference(new Mark(235, 247), buffer), new Scalar(buffer) { Value = new Mark(250, 267) } },
					{ new StaticReference(new Mark(270, 279), buffer), new Scalar(buffer) { Value = new Mark(282, 299) } },
					{ new StaticReference(new Mark(302, 313), buffer), new Scalar(buffer) { Value = new Mark(316, 334) } },
					{ new StaticReference(new Mark(337, 345), buffer), new Scalar(buffer) { Value = new Mark(348, 366) } },
					{ new StaticReference(new Mark(369, 381), buffer), new Scalar(buffer) { Value = new Mark(384, 389) } },
					{ new StaticReference(new Mark(392, 401), buffer), new Scalar(buffer) { Value = new Mark(404, 409) } },
					{ new StaticReference(new Mark(412, 425), buffer), new Scalar(buffer) { Value = new Mark(428, 444) } },
					{ new StaticReference(new Mark(447, 457), buffer), new Scalar(buffer) { Value = new Mark(460, 476) } },
					{ new StaticReference(new Mark(479, 493), buffer), new Scalar(buffer) { Value = new Mark(496, 512) } },
					{ new StaticReference(new Mark(515, 526), buffer), new Scalar(buffer) { Value = new Mark(529, 545) } },
					{ new StaticReference(new Mark(548, 559), buffer), new Scalar(buffer) { Value = new Mark(562, 565) } },
					{ new StaticReference(new Mark(568, 576), buffer), new Scalar(buffer) { Value = new Mark(579, 583) } },
					{ new StaticReference(new Mark(586, 601), buffer), new Scalar(buffer) { Value = new Mark(604, 621) } },
					{ new StaticReference(new Mark(624, 636), buffer), new Scalar(buffer) { Value = new Mark(639, 656) } },
					{ new StaticReference(new Mark(659, 672), buffer), new Scalar(buffer) { Value = new Mark(675, 695) } },
					{ new StaticReference(new Mark(698, 708), buffer), new Scalar(buffer) { Value = new Mark(711, 728) } },
					{ new StaticReference(new Mark(731, 742), buffer), new Scalar(buffer) { Value = new Mark(745, 745) } },
					{ new StaticReference(new Mark(748, 756), buffer), new Scalar(buffer) { Value = new Mark(759, 759) } },
				}
			};

			return primitiveTypeNode;
			//return googleApiNode;
		}

		public static T Deserialize<T>(ReadOnlyMemory<char> buffer)
		{
			ShamlAssigner<T> shamlAssigner = new(buffer);

			return shamlAssigner.Assign(default);
		}
	}
}