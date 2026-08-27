import { EyeOff, Lock, MonitorSmartphone, ShieldCheck } from "lucide-react";
import { Card } from "@/components/ui/card";

const points = [
  {
    icon: EyeOff,
    title: "No telemetry by default",
    desc: "Analytics are disabled out of the box. RailStrap doesn't phone home or report usage data anywhere.",
  },
  {
    icon: Lock,
    title: "Local-only credentials",
    desc: "The optional friend activity cookie is encrypted on-device with Windows DPAPI, and only ever sent directly to Roblox's own API.",
  },
  {
    icon: MonitorSmartphone,
    title: "Everything stays local",
    desc: "Playtime stats, settings, and logs live in your user profile. Nothing is uploaded to us.",
  },
  {
    icon: ShieldCheck,
    title: "Open source",
    desc: (
      <>
        Full source is public on{" "}
        <a
          href="https://github.com/Yoursel71/RailStrap"
          target="_blank"
          rel="noopener noreferrer"
          className="underline underline-offset-2"
        >
          GitHub
        </a>
        , so anyone can audit exactly what RailStrap does.
      </>
    ),
  },
];

export function Privacy() {
  return (
    <section id="privacy" className="py-24">
      <div className="container">
        <p className="mb-2.5 text-xs font-bold uppercase tracking-wider text-[hsl(217,100%,68%)]">
          Privacy
        </p>
        <h2 className="mb-3 text-[28px] font-extrabold tracking-tight">
          Nothing leaves your PC by default
        </h2>
        <p className="mb-11 max-w-[58ch] text-[15px] text-muted-foreground">
          RailStrap is fully open source, so every claim below is verifiable directly in the
          code.
        </p>

        <div className="grid grid-cols-1 gap-3.5 sm:grid-cols-2 lg:grid-cols-4">
          {points.map((p) => (
            <Card key={p.title}>
              <div className="mb-4 flex h-[38px] w-[38px] items-center justify-center rounded-[10px] border border-[#7c5cff]/20 bg-[#7c5cff]/10 text-[#a998ff]">
                <p.icon className="h-[19px] w-[19px]" strokeWidth={1.7} />
              </div>
              <h3 className="mb-1.5 text-[15px] font-bold tracking-tight">{p.title}</h3>
              <p className="text-[13.5px] leading-relaxed text-muted-foreground">{p.desc}</p>
            </Card>
          ))}
        </div>
      </div>
    </section>
  );
}
