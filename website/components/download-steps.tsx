import { ArrowDownToLine, ShieldCheck } from "lucide-react";
import { Button } from "@/components/ui/button";

const steps = [
  {
    title: "Download the latest release",
    desc: (
      <>
        from the{" "}
        <a
          href="https://github.com/Yoursel71/RailStrap/releases/latest"
          target="_blank"
          rel="noopener noreferrer"
          className="underline underline-offset-2"
        >
          GitHub Releases page
        </a>
        .
      </>
    ),
  },
  {
    title: "Run the installer.",
    desc: 'RailStrap ships unsigned, so SmartScreen may warn — click "More info", then "Run anyway".',
  },
  {
    title: "Install the .NET 6 Desktop Runtime",
    desc: "if prompted — RailStrap needs it to run.",
  },
  {
    title: "Configure your preferences",
    desc: "and finish setup. RailStrap is then available from your Start Menu.",
  },
];

export function DownloadSteps() {
  return (
    <section id="download" className="py-24">
      <div className="container">
        <p className="mb-2.5 text-xs font-bold uppercase tracking-wider text-[hsl(217,100%,68%)]">
          Install
        </p>
        <h2 className="mb-8 text-[28px] font-extrabold tracking-tight">
          Up and running in about a minute
        </h2>

        <div className="max-w-[820px] overflow-hidden rounded-2xl border border-border bg-card shadow-2xl shadow-black/20">
          {steps.map((s, i) => (
            <div
              key={s.title}
              className="flex items-start gap-4 border-b border-border bg-card px-5 py-[18px] last:border-b-0"
            >
              <div className="flex h-[26px] w-[26px] flex-none items-center justify-center rounded-md border border-[#7c5cff]/25 bg-[#7c5cff]/10 font-mono text-xs font-bold text-[#a998ff]">
                {i + 1}
              </div>
              <p className="text-sm text-muted-foreground">
                <strong className="font-semibold text-foreground">{s.title}</strong> {s.desc}
              </p>
            </div>
          ))}
        </div>

        <div className="mt-6 flex max-w-[820px] flex-col gap-4 rounded-2xl border border-[#7c5cff]/20 bg-[#7c5cff]/[0.06] px-5 py-5 text-[13.5px] text-muted-foreground sm:flex-row sm:items-center sm:justify-between">
          <div className="flex items-start gap-3">
            <ShieldCheck className="mt-0.5 h-[18px] w-[18px] flex-none text-[#a998ff]" />
            <div>
              <strong className="font-semibold text-foreground">Built to stay out of the game client.</strong><br />
              RailStrap wraps the launch process; it is not an exploit tool.
            </div>
          </div>
          <Button className="flex-none" asChild>
            <a href="https://github.com/Yoursel71/RailStrap/releases/latest" target="_blank" rel="noopener noreferrer">
              <ArrowDownToLine /> Get latest release
            </a>
          </Button>
        </div>
      </div>
    </section>
  );
}
