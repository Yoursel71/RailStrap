import { ReactNode } from "react";
import { Badge } from "@/components/ui/badge";

interface FeatureProps {
  badge?: string;
  title?: string;
  description?: string;
  visual?: ReactNode;
}

function Feature({
  badge = "Platform",
  title = "This is the start of something new",
  description = "Managing a small business today is already tough. Avoid further complications by ditching outdated, tedious trade methods. Our goal is to streamline SMB trade, making it easier and faster than ever.",
  visual,
}: FeatureProps) {
  return (
    <section className="w-full py-20 lg:py-28">
      <div className="container mx-auto">
        <div className="flex flex-col-reverse gap-10 lg:flex-row lg:items-center lg:gap-16">
          <div className="relative aspect-[989/573] w-full flex-1 overflow-hidden rounded-2xl border border-white/10 bg-[#111119] shadow-2xl shadow-black/30">
            {visual}
          </div>
          <div className="flex flex-1 flex-col gap-5">
            <div>
              <Badge>{badge}</Badge>
            </div>
            <div className="flex flex-col gap-4">
              <h2 className="max-w-xl text-left text-3xl font-extrabold tracking-tight md:text-5xl">
                {title}
              </h2>
              <p className="max-w-xl text-left text-base leading-7 text-muted-foreground lg:max-w-md">
                {description}
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

export { Feature };
