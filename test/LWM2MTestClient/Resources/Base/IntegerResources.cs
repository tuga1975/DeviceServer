﻿/***********************************************************************************************************************
 Copyright (c) 2016, Imagination Technologies Limited and/or its affiliated group companies.
 All rights reserved.

 Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
 following conditions are met:
     1. Redistributions of source code must retain the above copyright notice, this list of conditions and the
        following disclaimer.
     2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
        following disclaimer in the documentation and/or other materials provided with the distribution.
     3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote
        products derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
 USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
***********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoAP;
using CoAP.Server.Resources;
using Imagination.LWM2M;
using System.IO;

namespace Imagination.LWM2M.Resources
{
	internal class IntegerResources : LWM2MResources
	{
		public IntegerResources(String name)
			: base(name, true)
		{ }


		public static IntegerResources Deserialise(TlvReader reader)
		{
			IntegerResources result = null;
			if (reader.TlvRecord == null)
				reader.Read();
			if (reader.TlvRecord != null)
			{
				if (reader.TlvRecord.TypeIdentifier == TTlvTypeIdentifier.MultipleResources)
				{
					result = new IntegerResources(reader.TlvRecord.Identifier.ToString());
					if (reader.TlvRecord.Value != null)
					{
						using (TlvReader childReader = new TlvReader(reader.TlvRecord.Value))
						{
							while (childReader.Read())
							{
								if (childReader.TlvRecord.TypeIdentifier == TTlvTypeIdentifier.ResourceInstance)
								{
									IntegerResource childResource = new IntegerResource(childReader.TlvRecord.Identifier.ToString());
									childResource.Value = childReader.TlvRecord.ValueAsInt64();
									result.Add(childResource);
								}
							}
						}
					}
				}
			}
			return result;
		}

		public override void Serialise(TlvWriter writer)
		{
			ushort identifier;
			if (ushort.TryParse(Name, out identifier))
			{
				using (MemoryStream steam = new MemoryStream())
				{
					TlvWriter childWriter = new TlvWriter(steam);
					foreach (IntegerResource item in this.Children)
					{
						item.Serialise(childWriter, true);
					}
					writer.Write(TTlvTypeIdentifier.MultipleResources, identifier, steam.ToArray());
				}

			}
		}
	}
}
