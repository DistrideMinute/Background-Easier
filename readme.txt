Takes a folder as input.

Each subfolder counts as an instruction, with images having the innermost folder instruction applied first. Multiple images can be in the same subfolder.

If there is conflicts in naming images, it just doesn't do that image. Completed images are automatically deleted, and empty folders are automatically deleted.


Instructions:
stretch[-max][-w{width}][-h{height}] : stretches the image. By default, -w1920-h1080. -max prefers the image to be stretched larger. For example, stretch-w5-h4 on a 1x1 image results in a 4x4 image, while stretch-max-w5-h4 on the same image results in a 5x5 image. Stretch also shrinks. stretch-w2-h2 on a 4x8 image results in a 1x2 image, while stretch-max-w2-h2 on a 4x8 image results in a 2x4 image. Basically, -max takes the larger image allowable, no -max takes the smaller.

underlay[-r{R}-g{G}-b{B}] : underlays the image, flattening it onto a background of the colour specified with RGB. By default, underlay-r255-g255-b255.

expand[-w{width}][-h{height}][-t(op)/-b(ottom)][-l(eft)/-r(ight)] : expands the image to be at least widthxheight. If it is larger in a dimension, nothing changes. By default from the centre, -t-b-l-r affect it as in crop. It expands using transparent (alpha 0) colour. If unspecified, the sizes default to the image dimensions.

border[-t{thickness in pixels}][-r{R}-g{G}-b{B}] : creates a border around the image. By default, border-t3-r0-g0-b0. Note that the image resolution will increase by thickness*2 in both dimensions.

{other} : has no effect.

Note that spaces are escaped from commands too.

Summary:
stretch
underlay
expand
border
